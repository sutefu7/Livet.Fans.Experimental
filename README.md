# Livet.Fans.Experimental
Livet.Fans.Experimental は、非公式の Livet 拡張ライブラリです。
他の MVVM ライブラリの便利機能が Livet でも使えたら快適なのにと思い、実際に Livet 向けにカスタマイズして作成しました。
※公式提供のライブラリではありません。お問い合わせ含め、お間違いの無いようによろしくお願いいたします。

# 機能

## Livet 名前空間
NotificationObject, ViewModel の継承先クラス内で、Prism の SetProperty() を使えるようにしました。

    private string name;
    public string Name
    {
        get { return name; }
        set { this.SetProperty(ref name, value); }
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

IDisposable 型、またはそれを継承している型の拡張機能で、ReactiveProperty の AddTo() を使えるようにしました。ただし、ViewModel 継承先クラス内でのみ使用可能です。
※.NET Framework 4.7 環境で確認。

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

- Prism  
   Copyright (c) .NET Foundation  
   Released under the MIT license  
   https://github.com/PrismLibrary/Prism/blob/master/LICENSE  
   以下のソースを流用しています。  
   https://github.com/PrismLibrary/Prism/blob/master/Source/Prism/Commands/DelegateCommand.cs  
   https://github.com/PrismLibrary/Prism/blob/master/Source/Prism/Commands/PropertyObserver.cs  
   https://github.com/PrismLibrary/Prism/blob/master/Source/Prism/Mvvm/BindableBase.cs  

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

