using System.Windows;
using System.Windows.Controls;

namespace Validator
{
    internal class Validator
    {
        public static void bool TextboxValid(DependencyObject obj)
        {
            foreach (object child in LogicalTreeHelper.GetChildren(obj))
            {
                TextBox element = child as TextBox;
                if (element == null)
                    continue;
                if (Validation.GetHasError(element) || (element.Text.Length == 0))
                {

                }
            }
        }
    }
}
