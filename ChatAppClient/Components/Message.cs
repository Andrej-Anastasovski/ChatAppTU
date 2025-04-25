using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;

namespace ChatAppClient.Components
{
    class Message : Border
    {
        public Message(string text, string side)
        {

            if (side == "left" || side == "right")
            {


                Grid grid = new Grid();
                grid.Margin = new Thickness(0, 0, 0, 6);

                if (side == "right")
                {
                    grid.HorizontalAlignment = HorizontalAlignment.Right;
                }

                RowDefinition rowDefinition = new RowDefinition();
                rowDefinition.Height = GridLength.Auto;
                grid.RowDefinitions.Add(rowDefinition);

                ColumnDefinition columnDefinition = new ColumnDefinition();
                columnDefinition.Width = GridLength.Auto;
                grid.ColumnDefinitions.Add(columnDefinition);

                Border border = new Border();
                border.Background = new SolidColorBrush(Color.FromArgb(0xFF, 0x67, 0x33, 0x1B));
                border.CornerRadius = new CornerRadius(15);
                border.Margin = new Thickness(0, 0, 0, 0);


                TextBlock textBlock = new TextBlock();
                textBlock.Text = text;
                textBlock.MaxWidth = 400;
                textBlock.TextWrapping = TextWrapping.Wrap;
                textBlock.HorizontalAlignment = HorizontalAlignment.Left;
                textBlock.VerticalAlignment = VerticalAlignment.Stretch;
                textBlock.Background = Brushes.Transparent;
                textBlock.Foreground = Brushes.White;
                textBlock.Padding = new Thickness(10);
                textBlock.FontSize = 14;


                border.Child = textBlock;

                grid.Children.Add(border);


                Child = grid;

            }
            else
            {
                throw new InvalidOperationException("Message can be only on the left or on the right side!");
            }





        }
    }
}

