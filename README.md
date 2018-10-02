# Livet.Fans.Experimental
Livet.Fans.Experimental は、非公式の Livet 拡張ライブラリです。
他の MVVM ライブラリの便利機能が Livet でも使えたら快適なのにと思い、実際に Livet 向けにカスタマイズして作成しました。

※公式提供のライブラリではありません。お問い合わせ含め、お間違いの無いようによろしくお願いいたします。また、本ライブラリは Livet そのものを置き換えるものではありません。
本ライブラリの他 Livet も必要になります。

# 機能

## Livet 名前空間

NotificationObject, ViewModel の継承先クラス内で、Prism の SetProperty() を使えるようにしました。

    private string name;
    public string Name
    {
        get { return name; }
        set { this.SetProperty(ref name, value); }
    }

NotificationObject, ViewModel の継承先クラス内で、SetCommand() を使えるようにしました。SetProperty() のコマンド版です。※ListenerCommand&lt;T&gt; も可能です。

    private ViewModelCommand clickCommand;
    public ViewModelCommand ClickCommand
    {
        get
        {
            //if (clickCommand == null)
            //{
            //    //clickCommand = new ViewModelCommand(() => { });
            //    clickCommand = new ViewModelCommand(() => { }, () => true);
            //}
            //return clickCommand;

            // 上記と同等
            //return this.SetCommand(ref clickCommand, () => { });
            return this.SetCommand(ref clickCommand, () => { }, () => true);
        }
    }

ViewModelCommand, ListenerCommand&lt;T&gt; 型の拡張機能で、Prism の ObservesProperty(), ObservesCanExecute() を使えるようにしました。

ObservesProperty()

    public ViewModelCommand CheckCommand { get; set; }

    public MainWindowViewModel()
    {
        CheckCommand = new ViewModelCommand(() => { }, () => !string.IsNullOrEmpty(Name))
            .ObservesProperty(() => this.Name);
    }

ObservesCanExecute()

    private bool isChecked;
    public bool IsChecked
    {
        get { return isChecked; }
        set { this.SetProperty(ref isChecked, value); }
    }

    public ViewModelCommand CheckCommand { get; set; }

    public MainWindowViewModel()
    {
        CheckCommand = new ViewModelCommand(() => { })
            .ObservesCanExecute(() => this.IsChecked);
    }

先ほどの、SetCommand() でも、第4引数が ObservesProperty() 相当の役割となっているので、以下のような書き方も可能です。※ListenerCommand&lt;T&gt; も可能です。

    private ViewModelCommand clickCommand;
    public ViewModelCommand ClickCommand
    {
        get
        {
            //if (clickCommand == null)
            //{
            //    clickCommand = new ViewModelCommand
            //        (
            //            () => Console.WriteLine(Name),
            //            () => !string.IsNullOrEmpty(Name)
            //        ).ObservesProperty(() => Name);
            //}
            //return clickCommand;
            
            // 上記と同等
            return this.SetCommand(
                ref clickCommand, 
                () => Console.WriteLine(Name), 
                () => !string.IsNullOrEmpty(Name), 
                () => Name);
        }
    }

IDisposable 型、またはそれを継承している型の拡張機能で、ReactiveProperty の AddTo() を使えるようにしました。ただし、ViewModel 継承先クラス内でのみ使用可能です。
※ReactiveProperty の ObserveProperty() を使ったサンプルだったため、.NET Framework 4.7 環境で確認。

    class Person : NotificationObject
    {
        public string Name { get; set; }
    }

    public ReactiveProperty<string> Name { get; set; }

    public MainWindowViewModel()
    {
        var model = new Person() { Name = "taro" };
        Name = model
            .ObserveProperty(x => x.Name)
            .ToReactiveProperty()
            .AddTo(this);
    }

ViewModel の継承先クラス内で、MessageBox を簡単に呼び出せるようにしました。
情報、警告、エラー、確認（OK/Cancel, Yes/No, Yes/No/Cancel）の4種類のメッセージを、同期的に、または非同期的に表示することができます。
また、タイトルなどの関連情報は、Optional 引数で変更することが可能です。

    View
    <l:InteractionMessageTrigger Messenger="{Binding Messenger}" MessageKey="ShowInformationMessage">
        <l:InformationDialogInteractionMessageAction />
    </l:InteractionMessageTrigger>
    ※従来通り、Interaction.Triggers タグに含めて、それぞれ宣言

    ViewModel
    public async void Button_Click()
    {
        this.Messenger.Raise(new InformationMessage("情報だよ", "情報", MessageBoxImage.Information, "ShowInformationMessage"));
        await this.Messenger.RaiseAsync(new InformationMessage("情報だよ", "情報", MessageBoxImage.Information, "ShowInformationMessageAsync"));

        // 上記のような呼び出しと同等
        this.ShowInformationMessage("情報だよ");
        await this.ShowInformationMessageAsync("情報だよ");
    }

同様に、Window 処理系（Close(), Maximize(), Minimize(), Active()）も呼び出せるようにしました。

    View
    <l:InteractionMessageTrigger Messenger="{Binding Messenger}" MessageKey="Maximize">
        <l:WindowInteractionMessageAction />
    </l:InteractionMessageTrigger>
    ※従来通り、Interaction.Triggers タグに含めて、それぞれ宣言

    ViewModel
    public void Button_Click()
    {
        //Messenger.Raise(new WindowActionMessage(WindowAction.Maximize, "Maximize"));

        // 上記と同等
        this.Maximize();
    }

## Livet.Messaging 名前空間

ConfirmationMessage 型の拡張機能で、戻り値を MessageBoxResult 型で判断できるように戻しました。

    View
    <l:InteractionMessageTrigger Messenger="{Binding Messenger}" MessageKey="Confirm">
        <l:ConfirmationDialogInteractionMessageAction />
    </l:InteractionMessageTrigger>

    ViewModel
    // メッセージボックス（確認、Yes/No/Cancel）
    var mes = new ConfirmationMessage("確認だよ", "確認", MessageBoxImage.Question, MessageBoxButton.YesNoCancel, MessageBoxResult.Cancel, "Confirm");
    mes = await Messenger.GetResponseAsync(mes);

    //if (mes.Response == null)
    //    Console.WriteLine("Cancel 押した");

    //if (mes.Response.HasValue && mes.Response.Value)
    //    Console.WriteLine("Yes/OK 押した");

    //if (mes.Response.HasValue && !mes.Response.Value)
    //    Console.WriteLine("No 押した");

    // 上記と同等
    switch (mes.ClickedButton())     ←★これ
    {
        case MessageBoxResult.OK: Console.WriteLine("OK ボタンを押した"); break;
        case MessageBoxResult.Cancel: Console.WriteLine("Cancel ボタンを押した"); break;
        case MessageBoxResult.Yes: Console.WriteLine("Yes ボタンを押した"); break;
        case MessageBoxResult.No: Console.WriteLine("No ボタンを押した"); break;
    }


## Livet.Fans.Experimental 名前空間

XAML 側、標準 Binding にはできないバインド機構に関する拡張を組み込んでいます。あらかじめ、以下の名前空間を定義しておきます。

    xmlns:lf="http://schemas.livet-fans.jp/2018/wpf"

イベントに対する、メソッド直接バインディング、コマンド直接バインディングを使えるようにしました。

引数無し版

    ・View
    <StackPanel>
        <Button Content="button1" Click="{lf:Binding Button_Click}" />
        <Button Content="button2" Click="{lf:Binding ClickCommand}" />
    </StackPanel>

    ・ViewModel
    public ViewModelCommand ClickCommand { get; set; }

    public MainWindowViewModel()
    {
        ClickCommand = new ViewModelCommand(() => { });
    }

    public void Button_Click()
    {

    }

イベント引数を受け取る版（※イベント引数経由で View を扱えることもあるため、この仕様は検討中）

    ・View
    <StackPanel>
        <Button Content="button1" Click="{lf:Binding Button_Click, UseEventArgs=True}" />
        <Button Content="button2" Click="{lf:Binding ClickCommand, UseEventArgs=True}" />
    </StackPanel>

    ・ViewModel
    public ListenerCommand<EventArgs> ClickCommand { get; set; }

    public MainWindowViewModel()
    {
        ClickCommand = new ListenerCommand<EventArgs>((x) => { });
    }

    public void Button_Click(EventArgs e)
    {

    }

その他プロパティもバインド可能ですが、こちらはテスト段階です。

    <StackPanel>
        <TextBox Text="{lf:Binding Name, Mode=TwoWay, UpdateSourceTrigger=PropertyChanged}" />
    </StackPanel>

ReactiveProperty をバインドしている場合、ReactiveProperty の Value プロパティ省略指定時の、自動調整機能があります。
明示的に Value を記載している場合は、調整処理は働かず通常処理となります。

    ・View
    <StackPanel>
        <TextBox Text="{lf:Binding Name, Mode=TwoWay, UpdateSourceTrigger=PropertyChanged}" />
        <TextBox Text="{lf:Binding Name.Value, Mode=TwoWay, UpdateSourceTrigger=PropertyChanged}" />
    </StackPanel>

    ・ViewModel
    public ReactiveProperty<string> Name { get; set; }

    public MainWindowViewModel()
    {
        Name = new ReactiveProperty<string>();
    }

XAML 上で、Prism の ViewModelLocator を使えるようにしました。デフォルト値は「False」です。
注意点として、見つけた ViewModel は、引数無しコンストラクタ経由でインスタンス生成して DataContext にセットしていますので、引数ありコンストラクタを行いたい場合は従来通り、手動でバインドしてください。

    xmlns:lf="http://schemas.livet-fans.jp/2018/wpf"
    lf:ViewModelLocator.AutoWireViewModel="True"

以下、使用頻度の高いトリガーアクションについて、自動応答する MessengerOperator を使えるようにしました。デフォルト値は「False」です。
メッセージボックス系（情報、警告、エラー、確認）、Window 処理系（Close(), Maximize(), Minimize(), Active()）について、明示的に View 側で定義しなくても表示、または操作することが可能です。
ただし、これを書いたコントロール(の DataContext にバインドした ViewModel )でのみ有効であり、その対象のコントロールは Window だけです。注意点２つ目は後述します。

    View
    <Window x:Class="WpfApp24.Window1"
            ～
            xmlns:i="http://schemas.microsoft.com/expression/2010/interactivity"
            xmlns:l="http://schemas.livet-mvvm.net/2011/wpf"
            xmlns:lf="http://schemas.livet-fans.jp/2018/wpf"
            lf:ViewModelLocator.AutoWireViewModel="True"
            lf:MessengerOperator.AutoReceiveOperation="True"    ←★これ
            ～
            >
        
        <Grid>
            <Button Content="check" Click="{lf:Binding Button_Click}" Margin="100" />
        </Grid>
        
    </Window>

    ViewModel
    public class Window1ViewModel : ViewModel
    {
        public void Button_Click()
        {
            // メッセージボックス系（情報、警告、エラー、確認 / 同期、非同期呼び出し別）
            this.ShowInformationMessage("情報だよ");

            // Window 処理系（Close(), Maximize(), Minimize(), Active() / 同期、非同期呼び出し別）
            this.Maximize();
        }
    }

MessengerOperator の AutoReceiveOperation に True をセットすることで、以下と同等となります（Interaction.Triggers 内への定義が不要になる）。

    <Window x:Class="WpfApp24.Window1"
            ～
            xmlns:i="http://schemas.microsoft.com/expression/2010/interactivity"
            xmlns:l="http://schemas.livet-mvvm.net/2011/wpf"
            xmlns:lf="http://schemas.livet-fans.jp/2018/wpf"
            lf:ViewModelLocator.AutoWireViewModel="True"
            ～
            >
    
        <i:Interaction.Triggers>
    
            <!-- 表示のみ系のメッセージボックス -->
            <l:InteractionMessageTrigger Messenger="{Binding Messenger}" MessageKey="ShowInformationMessage">
                <l:InformationDialogInteractionMessageAction />
            </l:InteractionMessageTrigger>
    
            <!-- 
            ・・・
            種類に応じて、必要な分定義
            ・・・
            -->
    
            <!-- 戻り値あり系のメッセージボックス -->
            <l:InteractionMessageTrigger Messenger="{Binding Messenger}" MessageKey="Confirm">
                <l:ConfirmationDialogInteractionMessageAction />
            </l:InteractionMessageTrigger>
    
            <!-- 
            ・・・
            種類に応じて、必要な分定義
            ・・・
            -->
    
            <!-- Window 操作系 -->
            <l:InteractionMessageTrigger Messenger="{Binding Messenger}" MessageKey="Maximize">
                <l:WindowInteractionMessageAction />
            </l:InteractionMessageTrigger>
    
            <!-- 
            ・・・
            種類に応じて、必要な分定義
            ・・・
            -->
    
        </i:Interaction.Triggers>
        
        <Grid>
            <Button Content="check" Click="{lf:Binding Button_Click}" Margin="100" />
        </Grid>
        
    </Window>

また、以下の制限も追加であります。以下の機能を使いたい場合は、既存の実装をおこなってください。

- InteractionMessageTrigger  
   InvokeActionsOnlyWhileAttatchedObjectLoaded : bool の機能は無効です。  
   IsEnable : bool の機能は無効です。  

- InformationDialogInteractionMessageAction  
   InvokeActionOnlyWhenWindowIsActive : bool の機能は無効です。  

- ConfirmationDialogInteractionMessageAction  
   InvokeActionOnlyWhenWindowIsActive : bool の機能は無効です。  

- WindowInteractionMessageAction  
   InvokeActionOnlyWhenWindowIsActive : bool の機能は無効です。  

# 開発環境＆動作環境

本プログラムは、以下の環境で作成＆動作確認をおこなっています。

| 項目 | 値                                                               |
| ----- |:---------------------------------------------------- |
| OS   | Windows 10 Pro 1803 (64 bit)                              |
| IDE  | Visual Studio Community 2017                     |
| 言語 | C#                                                       |
| 種類 | クラスライブラリ (.NET Framework 4.5.2) |


# ライセンス

## 本プログラム

- Livet.Fans.Experimental  
   Copyright (c) sutefu7  
   Released under the MIT license  
   https://github.com/sutefu7/Livet.Fans.Experimental/blob/master/LICENSE  


## その他のプログラム

- Livet (LivetCask, 1.3.1.0)  
   Copyright (c) 2010-2011 Livet Project  
   Released under the zlib/libpng license  
   https://github.com/ugaya40/Livet/blob/master/license-jp.txt  
   以下のソースを流用しています。  
   https://github.com/ugaya40/Livet/blob/master/.NET4.0/Livet(.NET4.0)/EventListeners/WeakEvents/LivetWeakEventListener.cs  
   https://github.com/ugaya40/Livet/blob/master/.NET4.0/Livet(.NET4.0)/Behaviors/Messaging/InformationDialogInteractionMessageAction.cs  
   https://github.com/ugaya40/Livet/blob/master/.NET4.0/Livet(.NET4.0)/Behaviors/Messaging/ConfirmationDialogInteractionMessageAction.cs  
   https://github.com/ugaya40/Livet/blob/master/.NET4.0/Livet(.NET4.0)/Behaviors/Messaging/Windows/WindowInteractionMessageAction.cs  

- Prism  
   Copyright (c) .NET Foundation  
   Released under the MIT license  
   https://github.com/PrismLibrary/Prism/blob/master/LICENSE  
   以下のソースを流用しています。  
   https://github.com/PrismLibrary/Prism/blob/master/Source/Prism/Commands/DelegateCommand.cs  
   https://github.com/PrismLibrary/Prism/blob/master/Source/Prism/Commands/PropertyObserver.cs  
   https://github.com/PrismLibrary/Prism/blob/master/Source/Prism/Mvvm/BindableBase.cs  
   https://github.com/PrismLibrary/Prism/blob/master/Source/Wpf/Prism.Wpf/Mvvm/ViewModelLocator.cs  
   https://github.com/PrismLibrary/Prism/blob/master/Source/Prism/Mvvm/ViewModelLocationProvider.cs  

- ReactiveProperty  
   Copyright (c) 2018 neuecc, xin9le, okazuki  
   Released under the MIT license  
   https://github.com/runceel/ReactiveProperty/blob/master/LICENSE.txt  
   以下のソースを流用しています。  
   https://github.com/runceel/ReactiveProperty/blob/master/Source/ReactiveProperty.NETStandard/Extensions/IDisposableExtensions.cs  

- MarkupExtensionsForEvents  
   Copyright (c) sourcechord  
   Released under the MIT license  
   WPF4.5の新機能～「イベントのマークアップ拡張」で、イベント発生時のコマンド呼び出しをスッキリ記述する～  
   http://sourcechord.hatenablog.com/entry/2014/12/08/030947  
   https://github.com/sourcechord/MarkupExtensionsForEvents/blob/master/LICENSE  
   以下のソースを流用しています。  
   https://github.com/sourcechord/MarkupExtensionsForEvents/blob/master/MarkupExtensionsForEvents/InvokeCommandExtension.cs  

