using System;
using Shop.Node;

namespace Shop.Tests.Acceptance.API {
    class IsolatedShopNode : MarshalByRefObject, IDisposable
    {
        public ShopNode Node;

        public IsolatedShopNode()
        {
            Node = ShopNode.CreateDefault();
        }

        public void Start()
        {
            Node.Start();
        }

        public void Stop()
        {
            Node.Stop();
        }

        public void Dispose()
        {
            Node.Dispose();
        }
    }
}