using Microsoft.Xaml.Behaviors;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows.Controls;
using System.Windows.Input;

namespace iCustomerCareSystem.Utils
{
    public class DecimalInputBehavior : Behavior<TextBox>
    {
        private readonly Regex _regex = new Regex(@"^[+-]?\d*\.?\d*$");

        protected override void OnAttached()
        {
            base.OnAttached();
            AssociatedObject.PreviewTextInput += OnPreviewTextInput;
        }

        protected override void OnDetaching()
        {
            base.OnDetaching();
            AssociatedObject.PreviewTextInput -= OnPreviewTextInput;
        }

        private void OnPreviewTextInput(object sender, TextCompositionEventArgs e)
        {
            var textBox = sender as TextBox;
            if (textBox != null)
            {
                string newText = textBox.Text.Insert(textBox.CaretIndex, e.Text);
                e.Handled = !_regex.IsMatch(newText);
            }
        }
    }
}
