   M S B A M L   `   `   `  ���� ?  ;vCDR, Version=1.0.0.0, Culture=neutral, PublicKeyToken=nullclr-namespace:vCDRvCDR      vCDR.MainWindow   R NWindowsBase, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35W SPresentationCore, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35\ XPresentationFramework, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35D 9http://schemas.microsoft.com/winfx/2006/xaml/presentation    R NSystem.Xaml, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089:x,http://schemas.microsoft.com/winfx/2006/xaml     @d2http://schemas.microsoft.com/expression/blend/2008     Jmc;http://schemas.openxmlformats.org/markup-compatibility/2006     $localclr-namespace:vCDR     -   $
��1280��$	��720��
���   �  � WindowStartupLocation$  CenterScreen=�'  !System.Windows.Shell.WindowChrome   WindowChrome      GlassFrameThickness$ 0q�   CornerRadius$ 0��   CaptionHeight$ 0Q�.��� -   $��MainWindowGrid�� � RowDefinitions �� $
��Auto ��� $
��Auto ��� $��* �.+��� -   $��TitleBar��$��0��   TitleBarButtonColor#���   $��25��._��  � ColumnDefinitions �� $
��Auto ��� $
��Auto ��� $��* ��� $
��Auto ��� $
��Auto ��� $
��Auto �.+��� -    ��Name$ TitleBarTextBlock��$
��vCDR��$��0��  	TextColor#���  $��Consolas)�$��Light"�$��14(�$��Center=�$��5,0,0,0q��� $��1��$��Consolas)�$��Light"�$��14(�$��Center=�$��5,0,0,0q��� -   $D��>https://www.fly.faa.gov/rmt/cdm_operational_coded_departur.jsp?�  
BlueBorder#���   �� TextDecorations$ 	Underline{�.�	FAA CDR�� -   $ TilteBarZuluTime��$��	12:00:00Z��$��2��
���2�2�$��Consolas)�$��Light"�$��14(�$��Center=�$��5,0,0,0q��� -   	 ��Name$	 MinimizeButton��
 � ToolTip$
 Minimize��$��3��$��37��  WindowsButtonStyle#���  
 � Tag$0 *pack://application:,,,/Images/Minimize.png���� -   $	 MaximizeButton��$
 Maximize��$��4��$��37��#���  $0 *pack://application:,,,/Images/Maximize.png���� -	   $	 CloseButton��$
 Close��$��5��$��37��  CloseButtonStyle#���  $- 'pack://application:,,,/Images/Close.png���� $��1��
���   �$��50��$��Center=�._��  �� $
��Auto ��� $
��Auto ��� $
��Auto ��� $
��Auto ��� $
��Auto ��� $
��Auto ��� $
��Auto �.+��� $��Origin��$:
 43-4 character airport icao or 3 character ARTCC code��$��0��#���  $��Consolas)�$��Light"�$��16(�$��Center=�$��5,0,0,0q��� -
    ��Name$ OriginTextBox��$��1��$��25��$	��100��  GrayBackgroundLight#���  #���  $��Consolas)�$��Light"�$��16(�$��Center=�$
��Left=�$��5,0,0,0q�$��5,0,0,0q�  TextBoxStyle#���  .���� $��Destination��$:
 43-4 character airport icao or 3 character ARTCC code��$��2��#���  $��Consolas)�$��Light"�$��16(�$��Center=�$��10,0,0,0q��� -   $ DestinationTextBox��$��3��$��25��$	��100��#���  #���  $��Consolas)�$��Light"�$��16(�$��Center=�$
��Left=�$��5,0,0,0q�$��5,0,0,0q�#���  .���� $��	Fix/Route��$ 
 Any segment within a route��$��4��#���  $��Consolas)�$��Light"�$��16(�$��Center=�$��10,0,0,0q��� -   $ 
FixTextBox��$��5��$��25��$	��300��#���  #���  $��Consolas)�$��Light"�$��16(�$��Center=�$
��Left=�$��5,0,0,0q�$��5,0,0,0q�#���  .���� -   $	 SearchButton��$
 Search��$��6��$��37��$��25��$��Right=�$��10,0,0,0q�$. (pack://application:,,,/Images/Search.png��  SearchButtonStyle#���  �� $��2��$��5q�._�&   System.Windows.Controls.DataGrid  -   $��
RouteTable��   AutoGenerateColumns .     HeadersVisibility$ Column=�  GrayBackgroundDark#���  #���  $��Consolas)�
������ $��16(�   HorizontalGridLinesBrush# �     VerticalGridLinesBrush# �     RowBackground# �     CanUserAddRows .     SelectionUnit$
 Cell=�   SelectionMode$ Single=�   ColumnHeaderStyle ��  �� 
TargetType=  7System.Windows.Controls.Primitives.DataGridColumnHeader"  .���  �� Property � �� �� Value$ 25����  � �� 	 TitleBarColor# � 	 ��  � ��# �  ��  � ��$ Consolas)���  � ��$ 16(���  � ��$ Center=�   Columns 4  .System.Windows.Controls.DataGridTemplateColumn  ,  &System.Windows.Controls.DataGridColumn   
IsReadOnly .    Width5  /System.Windows.Controls.DataGridLengthConverter$	 125    Header �� $��Origin��
 _� $��Airport��   CellTemplate �� .R��� $
��Wrap=����� *Orig+   . $	 125  �� $��Destination��
 _� $��Airport�� �� .R��� $
��Wrap=����� *Dest+   . $	 125  �� $��Departure Fix��
 _� $!��Departure Fix (GATE/RV FIX)�� �� .R��� $
��Wrap=����� *DepartureFix+   . $	 100  �� $	��Nav��
 _� $*��$(1) BASIC | (2) RNAV | (3) Q Segment�� �� .R��� $
��Wrap=����� *Nav+   .  $ *  �� $��Routing��
 _� $��Coded Route�� �� .R��� $
��Wrap=����� *
CodedRoute+���� " �� �� Triggers ��  �� Binding �� *Coordination+  �� Value  N.a���  � ��
 �2�2���  �@�ToolTip$ 	Preferred��!   CellEditingTemplate! �� .R��� " �� TextWrapping$
" Wrap=����� *
CodedRoute+# �� UpdateSourceTrigger$# PropertyChanged=�.������ " �� ��  �� *Coordination+  N.a���  � ��
 �2�2���  �@�ToolTip$ 	Preferred��.��