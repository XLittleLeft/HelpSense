namespace HelpSense.API.Features.Pool
{
    public interface IPool<T>
    {
        public T Get();

        public void Return(T obj);
    }
}
