using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Media.Imaging;

namespace ChatAppClient.Components
{
    internal class ImageMessage : Border
    {

        public ImageMessage(BitmapImage newImage, string side)
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


                Image theImage = new Image();
                theImage.Source = newImage;
                theImage.MaxWidth = 300;


                border.Child = theImage;

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

