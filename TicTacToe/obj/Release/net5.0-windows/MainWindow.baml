   M S B A M L   `   `   `  ���� D  @TicTacToe, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null%clr-namespace:TicTacToe	TicTacToe      TicTacToe.MainWindow   W SPresentationCore, Version=5.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35\ XPresentationFramework, Version=5.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35e aSystem.Windows.Controls.Ribbon, Version=5.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089R NWindowsBase, Version=5.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35F 9http://schemas.microsoft.com/winfx/2006/xaml/presentation     R NSystem.Xaml, Version=5.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089:x,http://schemas.microsoft.com/winfx/2006/xaml     @d2http://schemas.microsoft.com/expression/blend/2008     Jmc;http://schemas.openxmlformats.org/markup-compatibility/2006     )localclr-namespace:TicTacToe       � Title$  Tic-Tac-Toe��$	��450��$	��800��$��Segoe UI Light)�   BackgroundColor#��[   � WindowStartupLocation$ CenterScreen=�.��(� .���  � RowDefinitions �� $
��auto ��� $��* �.+�� -    �Name$ TopPanel�����#121212$
��auto��$
��auto��.+��� -    ��Name$ 	TurnPanel�� �� Orientation$ 
Horizontal=�$
��Left=�$	��100��$��50��.+��� $��20(�$�� Player:��$��	0,10,0,10q�
��������� -    ��Name$ PlayerImage��$��Assets/X15.png��$��8,0,0,0q�$��25���� -    ��Name$ 
PlayerInfo��$
��Left=�$��Center=�$	��200��$��	126,0,0,0q�.+��� -    ��Name$ XButton��  RoundedButtonsNoMouseOver#��[ ���� .+��� -   	 ��Name$	 xStreakCount��
 �� Padding$
 15,0,0,0q�$�� X - ���� $��40��
������ �� -   $ OButton��#��[ ���� .+��� -   $	 oStreakCount��$
 15,0,0,0q�$�� O - ���� -	    ��Name$ 
GameCanvas��$��1��$	��300��$	��300��$��20q�.+�A� -
    A�Name$ GameGrid��$	��300��$	��300�� A� Rows$ 3�� A� Columns$ 3������  �� ImageSource$ Assets/Grid.png���� -    ��Name$ 
StrikeLine��  ButtonColor#��[ $��10�� �� StrokeStartLineCap$ Round=� �� StrokeEndLineCap$ Round=�$y�Hidden=�� -   $ 	EndScreen��$��2��#��[  $y�Hidden=�.+��� $ Vertical=�$��Center=�.+��� -   $ WinnerImage��$	��145��$	��200��$��Assets/X15.png���� -   $	 
ResultText��$��25(����#545454$��TIMES NEW ROMAN)�$��WINNER !��$��0,0,0,15q�$��Center=��� $ 
Horizontal=�$��Center=��� -   #��[ $��34(�$	��200��$��55��$��0,20,0,0q����� $	��200��.+���  �� TextAlignment$ Center=�$��
Play Again��