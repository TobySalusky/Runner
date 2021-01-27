using System;
using System.Collections.Generic;

namespace Runner {
    public static class UI {
        
        public static void transitionAll(IEnumerable<UIElement> elements, Func<UIElement, UITransition> transitionCreator) {
            foreach (var element in elements) {
                Runner.uiTransitions.Add(transitionCreator.Invoke(element));
            }
        }
        
    }
}