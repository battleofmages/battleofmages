Battle of Mages
====================

General info
---------------------

An open source MOBA, see https://battleofmages.com for more info.

Code Style Guide
---------------------

	using System;

	namespace Example { 
		public class Test {
			// Start
			void Start() {
				var x = 42;
				
				// Do nothing
				//DisabledCode();
			}
			
			// Update
			public void Update(string[] args) {
				for(int i = 0; i < 10; i++) {
					Console.WriteLine("{0}: Test", i);
				}
			}
		}
	}

* Indenting via __tabs__, aligning via __spaces__
* __Namespace__, __class__ and __function__ names are written in __PascalCase__
* __Variables__ are written in __camelCase__
* Always use `var` unless you can't or unless you really want to emphasize the data type
* Empty line separating the `using` block and the rest of code
* Empty line between 2 functions
* No empty lines at the start or end of the file
* Comments start with `//` followed by a space
* Disabled code starts with `//` __not__ followed by a space
* Always add a __comment directly above the function definition__ containing the __function name__ or an __explanation__
* One `using` declaration per line, no empty lines between them
* No spaces before the bracket of a function call or array index access
* One space before the opening bracket `{` of a block
* One space after the ending bracket `}` of a block if it is followed by a second block (e.g. `else`, `catch`, ...)
* Follow-up blocks like `else` and `catch` are written on the same line as the ending bracket `}` of the preceding block
* A comment `// ` that is not at the first line of a block needs to be preceded by a new line