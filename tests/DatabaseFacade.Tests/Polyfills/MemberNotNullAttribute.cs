#if NETFRAMEWORK
namespace System.Diagnostics.CodeAnalysis {
    // .NET Framework 沒有公開的 MemberNotNullAttribute，提供測試專案自用的定義讓 NRT 流程分析可用
    [AttributeUsage(AttributeTargets.Method | AttributeTargets.Property, Inherited = false, AllowMultiple = true)]
    internal sealed class MemberNotNullAttribute : Attribute {
        public MemberNotNullAttribute(string member) {
            Members = [member];
        }

        public MemberNotNullAttribute(params string[] members) {
            Members = members;
        }

        public string[] Members { get; }
    }
}
#endif
