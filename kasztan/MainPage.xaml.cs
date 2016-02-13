using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;
using Transforming.Common;
using Windows.Graphics.Display;

// The Blank Page item template is documented at http://go.microsoft.com/fwlink/?LinkId=391641

namespace kasztan
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class MainPage : Page
    {
        public MainPage()
        {
            this.InitializeComponent();

            this.NavigationCacheMode = NavigationCacheMode.Required;
        }

        /// <summary>
        /// Invoked when this page is about to be displayed in a Frame.
        /// </summary>
        /// <param name="e">Event data that describes how this page was reached.
        /// This parameter is typically used to configure the page.</param>
        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            // TODO: Prepare page for display here.
            DisplayInformation.AutoRotationPreferences = DisplayOrientations.Portrait;
            // TODO: If your application contains multiple pages, ensure that you are
            // handling the hardware Back button by registering for the
            // Windows.Phone.UI.Input.HardwareButtons.BackPressed event.
            // If you are using the NavigationHelper provided by some templates,
            // this event is handled for you.
        }

        private void NapisTextBox_KeyDown(object sender, KeyRoutedEventArgs e)
        {
            if (e.Key == Windows.System.VirtualKey.Enter)
            {
                string textToStack = NapisTextBox.Text;
                string[] wordsToStack = textToStack.Split(new char[] { ' ' });

                int maxHeight = (int)Math.Ceiling(MainGrid.ActualWidth);
                int maxWidth = (int)Math.Ceiling(MainGrid.ActualHeight);

                int fontSize = 120;

                WordsDistribution distribution = CreateProperWordsDistribution(fontSize, wordsToStack, maxWidth, maxHeight);
                TextStack.Children.Clear();
                distribution.Rows.Reverse();
                foreach (string row in distribution.Rows)
                {
                    TextBlock t = new TextBlock();
                    t.FontSize = distribution.FontSize;
                    t.Text = row;
                    //t.FontWeight = FontWeights.Bold;
                    t.TextAlignment = TextAlignment.Center;

                    LayoutTransformer lt = new LayoutTransformer();
                    lt.HorizontalAlignment = HorizontalAlignment.Stretch;
                    lt.VerticalAlignment = VerticalAlignment.Stretch;


                    lt.Content = t;
                    RotateTransform rt = new RotateTransform();
                    rt.Angle = 90;
                    lt.LayoutTransform = rt;
                    TextStack.Children.Add(lt);
                }


                TextStack.Visibility = Visibility.Visible;
                NapisInputContainer.Visibility = Visibility.Collapsed;
            }
        }

        private WordsDistribution CreateProperWordsDistribution(int fontSize, string[] words, int maxWidth, int maxHeight)
        {
            int currentFontSize = fontSize;
            Queue<string> wordQueue = new Queue<string>(words);
            List<string> rows = new List<string>();

            TextBlock t = new TextBlock();
            t.FontSize = currentFontSize;
            string textToRow = String.Empty;
            int finalHeight = 0;
            while (wordQueue.Count != 0)
            {
                string textToAppend = wordQueue.Peek();
                t.FontSize = currentFontSize;
                t.Text = textToRow + textToAppend + " ";
                t.Measure(new Size(maxWidth, maxHeight));
                int actualWidth = (int)Math.Ceiling(t.ActualWidth);
                int actualHeight = (int)Math.Ceiling(t.ActualHeight);

                if (actualWidth >= maxWidth)
                {
                    if (!String.IsNullOrEmpty(textToRow))
                    {
                        rows.Add(textToRow);
                    }
                    else
                    {
                        string[] check = textToAppend.Split(new char[] { ' ' });
                        if (check.Length <= 1)
                        {
                            currentFontSize -= 10;
                        }
                    }

                    textToRow = String.Empty;
                    finalHeight += actualHeight;

                }
                else
                {
                    textToAppend = wordQueue.Dequeue();
                    textToRow += textToAppend + " ";

                    if (wordQueue.Count == 0)
                    {
                        rows.Add(textToRow);
                        textToRow = String.Empty;
                        finalHeight += actualHeight;
                    }
                }
            }

            if (finalHeight > maxHeight && currentFontSize > 0)
            {
                return CreateProperWordsDistribution(currentFontSize - 10, words, maxWidth, maxHeight);
            }

            WordsDistribution wd = new WordsDistribution()
            {
                FontSize = currentFontSize,
                Rows = rows
            };

            return wd;

        }

        private class WordsDistribution
        {
            public int FontSize { get; set; }
            public List<string> Rows { get; set; }
        }



        private void TextStack_DoubleTapped(object sender, DoubleTappedRoutedEventArgs e)
        {
            TextStack.Visibility = Visibility.Collapsed;
            NapisInputContainer.Visibility = Visibility.Visible;
            NapisTextBox.Text = "";
        }

        private void Page_Loaded(object sender, RoutedEventArgs e)
        {

        }
    }
}
