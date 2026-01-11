using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Media;

namespace _1C11KS_MYAT_THADAR_LINN_MyCalculator
{
    public partial class MainWindow : Window
    {
        // --- Variables ---
        private double lastNumber = 0;
        private string selectedOperator = "";

        // --- Flags ---
        private bool isSoundOn = true;
        private bool isEnglishMode = true;
        private bool isNewCalculation = true; // Clears screen when starting a new number

        // --- Sound Player ---
        private SoundPlayer customSoundPlayer = new SoundPlayer("click_final.wav");

        public MainWindow()
        {
            InitializeComponent();
            try { customSoundPlayer.Load(); } catch { }
        }

        // --- 1. Window Dragging Logic ---
        private void Window_MouseDown(object sender, MouseButtonEventArgs e)
        {
            if (e.ChangedButton == MouseButton.Left)
                this.DragMove();
        }

        // --- 2. Sound Logic ---
        private void PlayClickSound()
        {
            if (isSoundOn)
            {
                try { customSoundPlayer.Play(); }
                catch { SystemSounds.Asterisk.Play(); }
            }
        }

        private void BtnSoundToggle_Click(object sender, RoutedEventArgs e)
        {
            isSoundOn = !isSoundOn;
            if (isSoundOn)
            {
                BtnSoundToggle.Content = "🔊 SOUND ON";
                PlayClickSound();
            }
            else
            {
                BtnSoundToggle.Content = "🔇 SOUND OFF";
            }
        }

        // --- 3. Language Logic ---
        private void BtnGlobe_Click(object sender, RoutedEventArgs e)
        {
            PlayClickSound();
            isEnglishMode = !isEnglishMode;
            ModeTextBlock.Text = isEnglishMode ? "Mode: English" : "Mode: Myanmar";
        }

        // --- 4. Number Buttons ---
        private void BtnNumber_Click(object sender, RoutedEventArgs e)
        {
            PlayClickSound();
            Button btn = (Button)sender;
            string number = btn.Content.ToString();

            if (DisplayTextBlock.Text == "0" || isNewCalculation)
            {
                DisplayTextBlock.Text = number;
                isNewCalculation = false;
            }
            else
            {
                DisplayTextBlock.Text += number;
            }
        }

        // --- 5. Decimal Point ---
        private void BtnDecimal_Click(object sender, RoutedEventArgs e)
        {
            PlayClickSound();
            if (isNewCalculation)
            {
                DisplayTextBlock.Text = "0.";
                isNewCalculation = false;
            }
            else if (!DisplayTextBlock.Text.Contains("."))
            {
                DisplayTextBlock.Text += ".";
            }
        }

        // --- 6. Operators (+, -, x, ÷) ---
       
        private void BtnOperation_Click(object sender, RoutedEventArgs e)
        {
            PlayClickSound();
            Button btn = (Button)sender;
            string newOperator = btn.Content.ToString();

            // 1. If there is already an operator pending (and we aren't just changing the operator)
            //    we must calculate the previous part first.
            if (selectedOperator != "" && !isNewCalculation)
            {
               
                PerformCalculation();
            }
            else
            {
                // Otherwise, just store the current number
                double.TryParse(DisplayTextBlock.Text, out lastNumber);
            }

            // 2. Set the new operator and get ready for the next number
            selectedOperator = newOperator;
            isNewCalculation = true;
        }

        // --- 7. Equals (=) ---
        private void BtnEqual_Click(object sender, RoutedEventArgs e)
        {
            PlayClickSound();
            // Perform the final calculation based on the last operator set
            PerformCalculation();

            // Clear operator so typing a number starts a fresh sequence, 
            // but keep the result on screen in case they want to operate on it.
            selectedOperator = "";
            isNewCalculation = true;
        }

        // --- HELPER: Performs the math ---
        private void PerformCalculation()
        {
            double newNumber;
            // Parse the number currently on the screen
            if (double.TryParse(DisplayTextBlock.Text, out newNumber))
            {
                switch (selectedOperator)
                {
                    case "+":
                        lastNumber = lastNumber + newNumber;
                        break;
                    case "-":
                        lastNumber = lastNumber - newNumber;
                        break;
                    case "x":
                        lastNumber = lastNumber * newNumber;
                        break;
                    case "÷":
                        if (newNumber == 0)
                        {
                            MessageBox.Show("Cannot divide by zero!", "Error");
                            return;
                        }
                        lastNumber = lastNumber / newNumber;
                        break;
                    default:
                        // If no operator is selected, the number just stays as is
                        lastNumber = newNumber;
                        break;
                }

                // Update Display and "lastNumber" for the next step
                DisplayTextBlock.Text = lastNumber.ToString();
            }
        }

        // --- 8. Percentage (%) ---
        private void BtnPercent_Click(object sender, RoutedEventArgs e)
        {
            PlayClickSound();
            double tempNumber;
            if (double.TryParse(DisplayTextBlock.Text, out tempNumber))
            {
                tempNumber = tempNumber / 100;
                // If we are in the middle of a calculation (e.g. 50 + 10%), calculate percentage of the base
                if (selectedOperator != "" && lastNumber != 0)
                {
                    tempNumber = lastNumber * tempNumber;
                }

                DisplayTextBlock.Text = tempNumber.ToString();
                // Note: We don't set isNewCalculation = true here, to allow chaining
            }
        }

        // --- 9. AC (All Clear) ---
        private void BtnAC_Click(object sender, RoutedEventArgs e)
        {
            PlayClickSound();
            DisplayTextBlock.Text = "0";
            lastNumber = 0;
            selectedOperator = "";
            isNewCalculation = true;
        }

        // --- 10. DEL (Delete) ---
        private void BtnDEL_Click(object sender, RoutedEventArgs e)
        {
            PlayClickSound();
            if (isNewCalculation) return;

            if (DisplayTextBlock.Text.Length > 0)
                DisplayTextBlock.Text = DisplayTextBlock.Text.Remove(DisplayTextBlock.Text.Length - 1, 1);

            if (DisplayTextBlock.Text == "")
                DisplayTextBlock.Text = "0";
        }

        // --- Keyboard Support ---
        private void Window_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Escape) BtnAC_Click(sender, null);
        }
    }
}



/*
================================================================================
【参考文献・コード解説 (References & Code Explanation)】

このプログラムで使用しているC#の構文やWPFの機能に関する参照先は以下の通りです。
(The references for C# syntax and WPF features used in this code are listed below.)

1. C#の基礎 (変数, IF文, Switch文, メソッド)
   [Basic C# Logic: Variables, If/Else, Switch, Methods]
   - 参照サイト: W3Schools C# Tutorial
   - URL: https://www.w3schools.com/cs/index.php
   - 該当箇所: 変数定義 (Variables), PerformCalculationメソッド内の計算ロジック

2. WPFフレームワーク (ウィンドウ操作, イベントハンドラ)
   [WPF Framework: Window Controls, Event Handlers]
   - 参照サイト: WPF-Tutorial.com / Microsoft Learn
   - URL: https://wpf-tutorial.com/
   - 該当箇所: BtnNumber_Click, Window_MouseDown などのUIイベント処理

3. クラス・メソッドの詳細 (Microsoft公式ドキュメント)
   [Official Documentation for Specific Classes]
   - SoundPlayer (音声再生 / Audio Playback): 
     https://learn.microsoft.com/ja-jp/dotnet/api/system.media.soundplayer
   
   - Double.TryParse (文字列から数値への変換 / Parsing Strings to Numbers): 
     https://learn.microsoft.com/ja-jp/dotnet/api/system.double.tryparse
   
   - String.Remove (文字列の削除・DEL機能 / Removing Characters): 
     https://learn.microsoft.com/ja-jp/dotnet/api/system.string.remove

================================================================================
*/