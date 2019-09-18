# YDock

## This is a layout system similar to visual studio.

The lastest version is based on .NET4.5.
if you use .NET4.0, You need to reference the Microsoft.Windows.Shell assembly to replace the WindowChrome class in .NET4.5.

The following is an application example of the framework:
![image](https://github.com/yzylovepmn/YDock/raw/master/YDock/Resource/Image/example.PNG)

The steps to add a layoutable window are as follows:

1.First instantiate a DockManager:

  var dockManager = new DockManager();
  
2.Create a class A that you want to add to the layout (must be inherited from UIElement);

3.Let class A implement the interface IDockSource

4.Register the class with dockManager, for example:

  var a = new A();
  
  dockManager.RegisterDock(a);
  
5.Finally, use the IDockSource interface member to display this class.
  a.DockControl.Show();
