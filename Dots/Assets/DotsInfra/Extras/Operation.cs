using System;

namespace Dots.Extras
{
    public class Operation<T> : IOperation<T>
    {
        private readonly T _result;

        private Operation(T result)
        {
            _result = result;
            IsSuccess = true;
        }

        private Operation()
        {
            IsSuccess = false;
        }

        public static Operation<T> Success(T result)
        {
            return new Operation<T>(result);
        }

        public static Operation<T> Failed()
        {
            return new Operation<T>();
        }

        public bool IsSuccess { get; private set; }

        public T Result
        {
            get
            {
                if (!IsSuccess)
                {
                    throw new InvalidOperationException("Trying to get result from invalid operation has been detected!");
                }

                return _result;
            }
        }
    }
}
