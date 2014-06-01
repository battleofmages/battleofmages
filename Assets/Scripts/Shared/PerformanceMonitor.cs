using UnityEngine;
//using System.Diagnostics;

public static class PerformanceMonitor {
	// Doesn't work for some reason
	/*private static PerformanceCounter cpuCounter;
	private static PerformanceCounter ramCounter;
	
	private static float lastCPUPoll;
	private static float lastRAMPoll;
	
	private static float _cpuUsage;
	private static float _freeRAM;
	
	// Contructor
	static PerformanceMonitor() {
		cpuCounter = new PerformanceCounter("Processor", "% Processor Time", "_Total");
		UnityEngine.Debug.Log(_cpuUsage = cpuCounter.NextValue());
		
		ramCounter = new PerformanceCounter("Memory", "Available MBytes");
		UnityEngine.Debug.Log(_freeRAM = ramCounter.NextValue());
	}
	
	// CPU usage in %
	public static float cpuUsage {
		get {
			if(Time.time - lastCPUPoll >= 1.0f) {
				_cpuUsage = cpuCounter.NextValue();
				UnityEngine.Debug.Log(_cpuUsage);
				lastCPUPoll = Time.time;
			}
			
			return _cpuUsage;
		}
	}
	
	// RAM usage in MB
	public static float freeRAM {
		get {
			if(Time.time - lastRAMPoll >= 1.0f) {
				_freeRAM = ramCounter.NextValue();
				lastRAMPoll = Time.time;
			}
			
			return _freeRAM;
		}
	}*/

	// FreeRAM
	public static void FreeRAM() {
		System.GC.Collect();
		Resources.UnloadUnusedAssets();
	}
}