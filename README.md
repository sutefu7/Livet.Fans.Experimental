# Livet.Fans.Experimental
Livet.Fans.Experimental は、非公式の Livet 拡張ライブラリです。
よりシンプルに、より快適にコーディングできるように、他の MVVM ライブラリの機能を Livet 向けにカスタマイズして組み込んでいます。
要するに各ライブラリの便利機能をパクりました。ただし、各ソース中にオリジナルのリンク先などの情報を残し、比較できるようにしています。

# 機能

## Livet 名前空間
NotificationObject, ViewModel の継承先クラス内で、Prism の SetProperty() を使えるようにしました。

    private string name;
    public string Name
    {
        get { return name; }
        set { this.SetProperty(ref name, value); }
    }

ViewModelCommand, ListenerCommand<T> で、Prism の ObservesProperty(), ObservesCanExecute() を使えるようにしました。

- ObservesProperty()

    public ViewModelCommand CheckCommand { get; set; }

    public MainWindowViewModel()
    {
        CheckCommand = new ViewModelCommand(() => { }, () => !string.IsNullOrEmpty(Name))
            .ObservesProperty(() => this.Name);
    }

- ObservesCanExecute()

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

IDisposable 型、またはそれを継承している型の変数について、ReactiveProperty の AddTo() を使えるようにしました。ただし、ViewModel 内でのみ使用可能です。
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

XAML 側で以下の名前空間を定義しておきます。

    xmlns:i="http://schemas.microsoft.com/expression/2010/interactivity"
    xmlns:l="http://schemas.livet-mvvm.net/2011/wpf"
    xmlns:lf="http://schemas.livet-fans.jp/2018/wpf"

イベントに対する、メソッド直接バインディング、コマンド直接バインディングを使えるようにしました。

- 引数無し版

    <StackPanel>
        <Button Content="button1" Click="{lf:Binding Button_Click}" />
        <Button Content="button1" Click="{lf:Binding ClickCommand}" />
    </StackPanel>

    public ViewModelCommand ClickCommand { get; set; }

    public MainWindowViewModel()
    {
        ClickCommand = new ViewModelCommand(() => { });
    }

    public void Button_Click()
    {

    }

- イベント引数を受け取る版

    <StackPanel>
        <Button Content="button1" Click="{lf:Binding Button_Click, UseEventArgs=True}" />
        <Button Content="button1" Click="{lf:Binding ClickCommand, UseEventArgs=True}" />
    </StackPanel>

    public ListenerCommand<EventArgs> ClickCommand { get; set; }

    public MainWindowViewModel()
    {
        ClickCommand = new ListenerCommand<EventArgs>((x) => { });
    }

    public void Button_Click(EventArgs e)
    {

    }

- その他プロパティもバインド可能ですが、こちらはテスト段階です。

    <StackPanel>
        <TextBox Text="{lf:Binding Name, Mode=TwoWay, UpdateSourceTrigger=PropertyChanged}" />
    </StackPanel>

- ReactiveProperty をバインドしている場合、ReactiveProperty の Value プロパティ省略指定時の、自動調整機能があります。
明示的に Value を記載している場合は、調整処理は働かず通常処理となります。

    <StackPanel>
        <TextBox Text="{lf:Binding Name, Mode=TwoWay, UpdateSourceTrigger=PropertyChanged}" />
        <TextBox Text="{lf:Binding Name.Value, Mode=TwoWay, UpdateSourceTrigger=PropertyChanged}" />
    </StackPanel>

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

- Prism
   Copyright (c) .NET Foundation  
   Released under the MIT license  
   https://github.com/PrismLibrary/Prism/blob/master/LICENSE  

- ReactiveProperty
   Copyright (c) 2018 neuecc, xin9le, okazuki  
   Released under the MIT license  
   https://github.com/runceel/ReactiveProperty/blob/master/LICENSE.txt  

