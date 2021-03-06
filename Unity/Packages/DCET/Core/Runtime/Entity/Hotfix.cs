﻿#if ILRuntime
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace DCET.Model
{
	public sealed class Hotfix : Object
	{
		public ILRuntime.Runtime.Enviorment.AppDomain appDomain = new ILRuntime.Runtime.Enviorment.AppDomain();		
		public Action Update;
		public Action LateUpdate;
		public Action OnApplicationQuit;

		private IStaticMethod start;
		private List<Type> hotfixTypes;

		public void GotoHotfix()
		{
			Log.Debug($"当前使用的是ILRuntime模式");

			hotfixTypes = appDomain.LoadedTypes.Values.Select(x => x.ReflectionType).ToList();
			start = new ILStaticMethod(appDomain, "DCET.Hotfix.Init", "Start", 0);
			start.Run();
		}

		public List<Type> GetHotfixTypes()
		{
			return hotfixTypes;
		}

		public void LoadHotfixAssembly(byte[] assBytes, byte[] pdbBytes)
		{
			appDomain.LoadAssembly(new MemoryStream(assBytes), new MemoryStream(pdbBytes), new ILRuntime.Mono.Cecil.Pdb.PdbReaderProvider());
		}

		public void LoadMainHotfixAssembly(byte[] assBytes, byte[] pdbBytes)
		{
			LoadHotfixAssembly(assBytes, pdbBytes);
		}
	}
}
#else
using System;
using System.Collections.Generic;
using System.Reflection;

namespace DCET.Model
{
	public sealed class Hotfix : Object
	{
		private Assembly assembly;
		private IStaticMethod start;
		private List<Type> hotfixTypes = new List<Type>();

		public Action Update;
		public Action LateUpdate;
		public Action OnApplicationQuit;

		public void GotoHotfix()
		{
			Log.Debug($"当前使用的是Mono模式");

			start = new MonoStaticMethod(assembly?.GetType("DCET.Hotfix.Init"), "Start");
			start.Run();
		}

		public List<Type> GetHotfixTypes()
		{
			return hotfixTypes;
		}

		public void LoadHotfixAssembly(byte[] assBytes, byte[] pdbBytes)
		{
			var assembly = Assembly.Load(assBytes, pdbBytes);

			if (assembly != null)
			{
				hotfixTypes.AddRange(assembly.GetTypes());
			}
		}

		public void LoadMainHotfixAssembly(byte[] assBytes, byte[] pdbBytes)
		{
			assembly = Assembly.Load(assBytes, pdbBytes);

			if (assembly != null)
			{
				hotfixTypes.AddRange(assembly.GetTypes());
			}
		}
	}
}
#endif