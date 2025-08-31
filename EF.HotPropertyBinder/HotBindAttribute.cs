namespace EF.HotPropertyBinder
{
    [AttributeUsage(AttributeTargets.Property, AllowMultiple = false, Inherited = true)]
    public class HotBindAttribute : Attribute
    {
    }

}