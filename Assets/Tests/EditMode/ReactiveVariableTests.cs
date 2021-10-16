using NUnit.Framework;
using Utils;

namespace Tests.EditMode
{
  public class ReactiveVariableTests
  {
    [Test]
    public void EmitsEventOnChange()
    {
      var variable = new ReactiveVariable<int>(17);
      var changed = false;

      void OnChange(object sender, ReactiveVariableEventArgs<int> reactiveVariableEventArgs)
      {
        changed = true;
      }

      variable.ValueChanged += OnChange;
      variable.CurrentValue += 3;
      variable.ValueChanged -= OnChange;
      
      Assert.That(changed, Is.True);
      Assert.That(variable.CurrentValue, Is.EqualTo(20));
    }
  }
}