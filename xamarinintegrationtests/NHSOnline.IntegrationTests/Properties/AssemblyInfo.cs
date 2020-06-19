using Microsoft.VisualStudio.TestTools.UnitTesting;

[assembly: Parallelize(Workers = 3, Scope = ExecutionScope.MethodLevel)]