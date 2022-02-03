namespace Dots.Extras
{
    public interface IOperation<out T>
    {
        bool IsSuccess { get; }
        T Result { get; }
    }
}