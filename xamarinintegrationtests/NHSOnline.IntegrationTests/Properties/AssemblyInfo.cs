using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;

[assembly: CLSCompliant(false)]
[assembly: Parallelize(Workers = 7, Scope = ExecutionScope.MethodLevel)]