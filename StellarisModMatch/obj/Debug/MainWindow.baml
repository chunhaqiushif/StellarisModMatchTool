   M S B A M L   `   `   `  ����P  LStellarisModMatchTool, Version=0.1.7.0, Culture=neutral, PublicKeyToken=null5clr-namespace:StellarisModMatchStellarisModMatch  5   	   "    StellarisModMatch.MainWindow   5   	   \ XPresentationFramework, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35R NWindowsBase, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35W SPresentationCore, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35D 9http://schemas.microsoft.com/winfx/2006/xaml/presentation    5   	   R NSystem.Xaml, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089:x,http://schemas.microsoft.com/winfx/2006/xaml     5   	   @d2http://schemas.microsoft.com/expression/blend/2008     5   	   Jmc;http://schemas.openxmlformats.org/markup-compatibility/2006     5   	   1localclr-namespace:StellarisModMatch     5   	   -   5          � Title$1  +群星Stellaris Mod匹配工具 - Ver 0.1.8��5   	    � 
ResizeMode$ CanMinimize=�5	   	   $
��1280��67   $	��600��6D   $��Center=�6Q   $��Center=�6n   $	��800��6�   $	��600��6�   $y�Visible=�6�    � Icon$$ Resources/exe_icon_bigsize.bmp��6�    � WindowStartupLocation$ CenterScreen=�6�    � 	Resources 5
         contentCenterStyle �� 
TargetType �� Property �� Value��%V   &	        �� 5   
   " ��6+   .�5      ��  � ��$ Center=�65   5      5      .��5      � $	��572��6    � RowDefinitions 5   
   �� 5      	 � ColumnDefinitions	 5   
   �� 5      $
��173* �6   �� 5      $��1100* �6   .+�5   
   &   System.Windows.Controls.DataGrid  -   5        
  Name$
 ModDataView��5         
IsReadOnly .    FrozenColumnCount$ 1��6^   4  .System.Windows.Automation.AutomationProperties   IsColumnHeader .    AutoGenerateColumns .   J� RenderTransformOrigin$ 	0.571,0.6&�6�   $   HorizontalScrollBarVisibility$ Hidden=�6�   $��1��6  $��
0,10,10,20q�6     SelectionMode$ Single=�6(  $��Microsoft YaHei)�6?  ��6(   �� *ModDataList+   RowStyle 5      �� 5      )  #System.Windows.Controls.DataGridRow"  6   .�5      ��  � ��
 �����63    �� Triggers 5      P� 5       P� Property �@J�IsMouseOver P� Value . . �5      ��  � ��
 �����6;   5       P� 5!       �@ 
IsSelected . . �5"      ��  � ��
 �����6;   �� 5#       � ��
 �  ��6;   5$      5&         Columns 5(      4  .System.Windows.Controls.DataGridTemplateColumn  5)      ,  &System.Windows.Controls.DataGridColumn   HeaderTemplate 5*      �� 5+      .R�5,      ��  ��Name$ checkAllItem��6'   -   5        $��Center=�5,   Y   $��Center=�6t   5-         CellTemplate 5/      �� 50      .R�51      �� $ 	itemCheck��6'   -   5        $��Center=�51   �   $��Center=�6�    d� 	IsChecked 6:   �� *
IsSelected+ �� Mode$ TwoWay=� �� UpdateSourceTrigger$ PropertyChanged=�6�   52      54      0  *System.Windows.Controls.DataGridTextColumn  58         Header$ ID��6<   1  +System.Windows.Controls.DataGridBoundColumn   Binding 6%   �� *Id+6<     59      $ 名称��6>      Width5  /System.Windows.Controls.DataGridLengthConverter$	 .5* 6J    6%   �� *Name+6J     5:      $ 	Mod类型��6>    6%   �� *Type+6>     5;      $ 对应版本��6A       ElementStyle#  [  6O    6%   �� *	Version+6O     5<      $ 本机存在��6B   #  [  6Q    6%   �� *
_IsExist+6Q     5=      $ 当前状态��6P   #  [  6^    6%   �� *	_IsEnable+$ TwoWay=�6^   5	  /System.Windows.Controls.DataGridHyperlinkColumn	  5>      $ 社区地址��6j    6*   �� *Http+! 	  ContentBinding! 6D   �� *
LinkName+  5?      �� 5@      " ��6    .�5A      -   5        �� 5B       � ��$ Center=�6D   5C      5E      5G      �� 5H   
   -   5        " ��Name$" EnableModButton��5H      $��启用当前选择Mod��6*   $
��Left=�6>   $��	10,38,0,0q�6Y   $	��Top=�6l   $	��153��6�   $��30��6�   # J� Effect# 5I      3
  -System.Windows.Media.Effects.DropShadowEffect
  5J      $ 
  	Direction$$ 0Q�6#   % 
  Opacity$
% 0.45Q�61   & 
  ShadowDepth$& 0Q�6@   5L      �� 5M   
   -   5        $" SaveModButton��5M      $!��保存当前选择Mod列表��6(   $
��Left=�6>   $��	10,73,0,0q�6Y   $	��Top=�6l   $	��153��6�   $��30��6�   # 5N      
  5O      $$ 0Q�6#   $
% 0.45Q�61   $& 0Q�6@   5Q      �� 5R   
   -   5        $" LoadModButton��5R      $��加载Mod列表��6(   $
��Left=�6:   $��
10,108,0,0q�6U   $	��Top=�6i   $	��153��6�   $��30��6�   # 5S      
  5T      $$ 0Q�6#   $
% 0.45Q�61   $& 0Q�6@   5V      �� 5W   
   -	   5        $" RefleshModListButton��5W      $!��恢复已启用Mod的选择��6/   $
��Left=�6E   $��
10,143,0,0q�6`   $	��Top=�6t   $	��153��6�   $��30��6�   # 5X      
  5Y      $$ 0Q�6#   $
% 0.45Q�61   $& 0Q�6@   5[      >� 5\   
   -
   5        ' >�Name$' 
PressStart��5\      .��5^      �� -   5        ( ��Name$
( Logo��5^      $	��153��6#   $��
10,0,10,20q�60   $	��153��6D   $��Bottom=�6P   ��5_      �� 5`      " ��6   .�5a      ��  � ��$ Resources\logo-normal.png��63    5b      P� 5c       �@J�IsMouseOver . . �5d   "   ��  � ��$" Resources\logo-mouseover.png��6;   5e      5g      # 5i      
  5j      $$ 0Q�6'   $	% 0.8Q�65   $& 0Q�6C   ) 
  
BlurRadius$) 10Q�6S   5l      5m      �� 5o   
   -   5        $��	InputText��5o      $
��Left=�6#   $��23��6>   $��	10,10,0,0q�6J   * �� TextWrapping$
* Wrap=�6]   $	��Top=�6q   $��Center=�6�   $	��153��6�   .��6�    5q        	HelpBrush+ j� TileMode, �� Opacity- j� Stretch. j� 
AlignmentX/ !� Visual��%�   &	       !� 5r      $
+ None=�60   $	, 0.3Q�6@   $
- None=�6N   $. Center=�6]   / 5s      �� 5t      $��根据Mod名查找��6$   5v      5w      .��6!   ��5x      �� 5y      " ��6    5z      P� 5{       � �� 62   Z� . �5|      ��  � ��# [ 6;   5}      P� 5~       � ��$  ��62   . �5      ��  � ��# [ 6;   5�      5�      .��5�      # 5�      
  5�      $$ 0Q�6#   $& 0Q�61   $
% 0.45Q�6A   .��5�      5�      5�      5�      