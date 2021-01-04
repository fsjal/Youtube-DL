using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using Youtube_DL.Entities;

namespace Youtube_DL.Behaviors
{
    class ListItemClickBehavior
    {
        public static ICommand GetCommand(DependencyObject obj)
        {
            return (ICommand)obj.GetValue(CommandProperty);
        }

        public static void SetCommand(DependencyObject obj, ICommand value)
        {
            obj.SetValue(CommandProperty, value);
        }

        public static readonly DependencyProperty CommandProperty =
            DependencyProperty.RegisterAttached("Command", typeof(ICommand), typeof(ListItemClickBehavior),
                new PropertyMetadata(null, OnCommandPropertyChanged));

        private static void OnCommandPropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            
            ListViewItem element = (ListViewItem)d;
            if (element != null)
            {
                if ((e.NewValue != null) && (e.OldValue == null))
                {
                    element.MouseDoubleClick += (s, e) => OnItemDoubleClick(element);
                }
                else if ((e.NewValue == null) && (e.OldValue != null))
                {
                    element.MouseDoubleClick -= (s, e) => OnItemDoubleClick(element);
                }
            }
        }

        public static Link GetLink(DependencyObject obj)
        {
            return (Link)obj.GetValue(LinkProperty);
        }

        public static void SetLink(DependencyObject obj, Link value)
        {
            obj.SetValue(LinkProperty, value);
        }

        public static readonly DependencyProperty LinkProperty =
            DependencyProperty.RegisterAttached("Link", typeof(Link), typeof(ListItemClickBehavior),
                new PropertyMetadata());

        private static void OnItemDoubleClick(ListViewItem element)
        {
            ICommand command = GetCommand(element);
            Link link = GetLink(element);
            command.Execute(link);
        }
    }
}
