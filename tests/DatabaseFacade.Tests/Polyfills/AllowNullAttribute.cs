#if NETFRAMEWORK
namespace System.Diagnostics.CodeAnalysis {
    // .NET Framework 沒有公開的 AllowNullAttribute，提供測試專案自用的定義讓 NRT 流程分析可用
    [AttributeUsage(AttributeTargets.Field | AttributeTargets.Parameter | AttributeTargets.Property, Inherited = false)]
    internal sealed class AllowNullAttribute : Attribute {
    }
}
#endif
