   M S B A M L   `   `   `  ����  MonPlotterWPF    MonPlotterWPF.MainWindow   5   	   \ XPresentationFramework, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35R NWindowsBase, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35W SPresentationCore, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35D 9http://schemas.microsoft.com/winfx/2006/xaml/presentation    5   	   R NSystem.Xaml, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089:x,http://schemas.microsoft.com/winfx/2006/xaml     5   	   @d2http://schemas.microsoft.com/expression/blend/2008     5   	   Jmc;http://schemas.openxmlformats.org/markup-compatibility/2006     5   	   R NOxyPlot.Wpf, Version=1.0.0.0, Culture=neutral, PublicKeyToken=75e952ba404cdbb0 oxyhttp://oxyplot.org/wpf  5   	     � Title$  
MainWindow��5   	   $	��350��6   $	��525��6)   .��5	      �  � ShowGridLines .   � ColumnDefinitions 5
   
   �� 5      $��* �6   �� 5      $��3* �6   �� 5      $	��16* �6    � RowDefinitions 5   
   �� 5      $��5* �6   �� 5      $��5* �6   �� 5      $��* �6   �� 5      $��* �6   �� 5      $��2* �6   �� 5      $��* �6   .+�5   
     OxyPlot.Wpf.PlotView  $��2��6   $��6��5         Model 5      �� *	Modèle+�� 5   
   -   5        $��listBoxGrandeurs��5      $��0��5      $��0��5      $��2��5       �� SelectionMode$ Multiple=�5      ��5!      �� *	Grandeurs+�� 5"   
   -   5        $��listBoxCapteurs��5"      $��1��5#      $��0��5$      $��2��5%      $ Multiple=�5&      5'      �� 5(   
   $��2��6   $��0��5)      $��Du��5*      (  "System.Windows.Controls.DatePicker  5+   
   -   5        $��2��5+      $��1��5,         SelectedDate 5.      �� *DebutFiltre+�� 5/   
   $��3��6   $��0��50      $��Au��51        52   
   -   5        $��3��52      $��1��53       55      �� *	FinFiltre+�� 56   
   -   5        $��4��56      $��0��57      $��2��58      ��5:      �� *
Analyses+ �� SelectedItem 5;      �� *AnalyseSélectionnée+5<      5=      