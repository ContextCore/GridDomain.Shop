using Autofac;

namespace Shop.Composition {
    public static class UnityEtensions
    {
        public static void RegisterType<TAbstr, TImpl>(this ContainerBuilder builder)
        {
            builder.RegisterType<TImpl>().
                    As<TAbstr>();
        }
    }
}