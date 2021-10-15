//namespace Abstractions
//{
//    public interface IWriteOperation<TCommand> // IRequest<TCommand>
//    {
//        void PerformOperation(TCommand operation);
//    }
//}

namespace Abstractions
{
    public interface ITest<TCommand>
    {
        void PerformOperation(TCommand operation);

    }
}
