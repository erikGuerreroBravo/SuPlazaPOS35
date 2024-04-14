using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DsiCodeTech.Builder.Generic
{
    public  class Builder<TObject> : BuilderBase<TObject, Builder<TObject>>
    {
        public Builder()
        {
        }

        public Builder(Func<TObject> factory)
        : base(factory)
        {
        }
        public static Builder<TObject> New => new();
        public static Builder<TObject> Auto => new Builder<TObject>().WithAutoProperties();
        public static Builder<TObject> Uninitialized => new(Factories.Uninitialized<TObject>);
        public static Builder<TObject> From(Func<TObject> factory) => new(factory);
    }
}
