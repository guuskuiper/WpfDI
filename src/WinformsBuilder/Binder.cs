using System.ComponentModel;
using System.Linq.Expressions;
using System.Reflection;

namespace WinformsBuilder;

// https://stackoverflow.com/questions/671968/retrieving-property-name-from-lambda-expression
public static class Binder
{
    public static void Bind<TControl, TViewModel>(this TControl control, string controlPropertyName,
        TViewModel vm, string viewModelPropertyName)
        where TControl : Control
        where TViewModel : INotifyPropertyChanged
    {
        var binding = new Binding(controlPropertyName, vm, viewModelPropertyName);
        control.DataBindings.Add(binding);
    }

    public static void Bind<TControl, TControlProperty, TViewModel, TViewModelProperty>(this TControl control, Expression<Func<TControl, TControlProperty>> propertyLambda,
        TViewModel vm, Expression<Func<TViewModel, TViewModelProperty>> vmPropertyLambda)
        where TControl : Control
        where TViewModel : INotifyPropertyChanged
    {
        string vmPropertyName = GetPropertyName(vmPropertyLambda);
        string controlPropertyName = GetPropertyName(propertyLambda);
        Bind(control, controlPropertyName, vm, vmPropertyName);
    }

    public static PropertyInfo GetPropertyInfo<TSource, TProperty>(TSource source, Expression<Func<TSource, TProperty>> propertyLambda)
    {
        return GetPropertyInfo(propertyLambda);
    }

    public static string GetPropertyName<TSource, TProperty>(this TSource source, Expression<Func<TSource, TProperty>> propertyLambda)
    {
        PropertyInfo prodInfo = GetPropertyInfo(propertyLambda);
        return prodInfo.Name;
    }

    public static string GetPropertyName<TSource, TProperty>(Expression<Func<TSource, TProperty>> propertyLambda)
    {
        PropertyInfo prodInfo = GetPropertyInfo(propertyLambda);
        return prodInfo.Name;
    }

    public static PropertyInfo GetPropertyInfo<TSource, TProperty>(Expression<Func<TSource, TProperty>> propertyLambda)
    {
        if (propertyLambda.Body is not MemberExpression member)
        {
            throw new ArgumentException($"Expression '{propertyLambda}' refers to a method, not a property.");
        }

        if (member.Member is not PropertyInfo propInfo)
        {
            throw new ArgumentException($"Expression '{propertyLambda}' refers to a field, not a property.");
        }

        Type type = typeof(TSource);
        if (propInfo.ReflectedType != null && type != propInfo.ReflectedType && !type.IsSubclassOf(propInfo.ReflectedType))
        {
            throw new ArgumentException($"Expression '{propertyLambda}' refers to a property that is not from type {type}.");
        }

        return propInfo;
    }
}