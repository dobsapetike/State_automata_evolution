using System;
using System.ComponentModel;
using System.Linq.Expressions;

namespace BenchmarkDepot.Classes.ViewModel
{

    public class BaseViewModel : INotifyPropertyChanged
    {

        public event PropertyChangedEventHandler PropertyChanged;

        protected virtual void RaisePropertyChanged(Expression<Func<object>> expression)
        {
            var handler = PropertyChanged;
            if (handler == null) return;
            if (expression.NodeType != ExpressionType.Lambda)
            {
                throw new ArgumentException("Value must be a lambda expression");
            }
            var body = expression.Body as MemberExpression;
            if (body == null) return;
            var propertyName = body.Member.Name;
            handler(this, new PropertyChangedEventArgs(propertyName));
        }

    }

}
