   M S B A M L   `   `   `  ����F  BAlgorytmika, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null9!clr-namespace:Algorytmika.WindowsAlgorytmika.Windows  5   	   2    ,Algorytmika.Windows.RouteConfigurationWindow   5   	   \ XPresentationFramework, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35W SPresentationCore, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35R NWindowsBase, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35D 9http://schemas.microsoft.com/winfx/2006/xaml/presentation    5   	   R NSystem.Xaml, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089:x,http://schemas.microsoft.com/winfx/2006/xaml     5   	   @d2http://schemas.microsoft.com/expression/blend/2008     5   	   Jmc;http://schemas.openxmlformats.org/markup-compatibility/2006     5   	   3local!clr-namespace:Algorytmika.Windows     5   	     � SizeToContent$  Height=�5   	    � Title$ RouteConfigurationWindow��5	   	   $	��300��6*   $	��300��67   .��5
      �  � RowDefinitions 5   
   �� 5      $
��Auto �6   �� 5      $��* �6   �� 5      $
��Auto �6    � ColumnDefinitions 5   
   �� 5      $��* �6   �� 5      $
��Auto �6   �� 5      $
��Auto �6   .+�5   
   �� $��Ustawienia trasy��6   $��0��6-   $��0��6:   $��3��6J   .��5      �  5      �� 5      $
��Auto �6$   �� 5      $
��Auto �6$   �� 5       5      �� 5      $
��Auto �6'   �� 5      $��* �6'   .+�5!      �� $��Limit długości trasy: ��6   $��0��69   $��0��6F   $��5q�6V   $��5q�6a   �� 5"      $��Miasto początkowe ��6   $��1��65   $��0��6B   $��5q�6R   $��5q�6]   �� 5$      -   5        $��	DistanceL��5$   7   $��0��6   $��1��6'   $��5q�6H   $��5q�6S   $��2��6_   �� 5%      -   5        $��citiesComboBox��5%   c   $��1��6   $��1��6(   $��5q�68   $��5q�6C   $��2��6O   6c   5&      5'      �� 5)   
   -   5        $��Oblicz��5)      $��2��6"   $��1��6/   $��5q�6?   $��5q�6J   6V   �� 5*   
   -   5        $��Anuluj��5*      $��2��6"   $��2��6/   $��5q�6?   $��5q�6J   6V   5/      50      