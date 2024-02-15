using ADOFAI;
using HarmonyLib;
using TMPro;
using UnityEngine;

namespace ShowDecoCount {
    [HarmonyPatch]
    public class DecorationPatch {

        private static TMP_Text _text;
        
        [HarmonyPatch(typeof(InspectorPanel), "ShowPanel")]
        [HarmonyPostfix]
        public static void ShowPanel(LevelEventType eventType, RectTransform ___tabs) {
            if(eventType != LevelEventType.DecorationSettings) return;
            foreach(Component tab in ___tabs) {
                InspectorTab component = tab.gameObject.GetComponent<InspectorTab>();
                if(component == null || eventType != component.levelEventType) continue;
                int count = ADOBase.editor.decorations.Count;
                _text = component.panel.title;
                _text.text = RDString.Get("editor." + eventType) + $" ({count})";
            }
        }
        
        [HarmonyPatch(typeof(scnEditor), "AddDecoration", typeof(LevelEventType), typeof(int))]
        [HarmonyPostfix]
        public static void AddDecoration() {
            if(_text == null) return;
            int count = ADOBase.editor.decorations.Count;
            _text.text = RDString.Get("editor." + LevelEventType.DecorationSettings) + $" ({count})";
        }
        
        [HarmonyPatch(typeof(scnEditor), "RemoveEvent", typeof(LevelEvent), typeof(bool))]
        [HarmonyPostfix]
        public static void RemoveEvent(LevelEvent evnt) {
            if(evnt == null || !evnt.IsDecoration || _text == null) return;
            int count = ADOBase.editor.decorations.Count;
            _text.text = RDString.Get("editor." + LevelEventType.DecorationSettings) + $" ({count})";
        }
    }
}