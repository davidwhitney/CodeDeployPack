using System;
using System.Reflection;

namespace CodeDeployPack.Test.Unit.TestDoubles
{
    public class FakeAssembly : Assembly
    {
        private readonly AssemblyInformationalVersionAttribute _informationalVersion;
        private readonly Version _version;

        public FakeAssembly(Version version)
        {
            _version = version;
        }

        public FakeAssembly(AssemblyInformationalVersionAttribute informationalVersion)
        {
            _informationalVersion = informationalVersion;
        }

        public FakeAssembly(Version version, AssemblyInformationalVersionAttribute informationalVersion)
        {
            _version = version;
            _informationalVersion = informationalVersion;
        }

        public override AssemblyName GetName()
        {
            return new AssemblyName{Version = _version};
        }

        public override object[] GetCustomAttributes(Type attributeType, bool inherit)
        {
            if (attributeType != typeof(AssemblyInformationalVersionAttribute)) throw new NotImplementedException();

            return new Attribute[] {_informationalVersion};
        }
    }
}