using System.Linq;
using ADOFAI;
using HarmonyLib;

namespace ShowDecoCount {
    [HarmonyPatch]
    public class EventPatch {
        private static LevelEventCategory? _lastEventCategory;
        
        [HarmonyPatch(typeof(CategoryTab), "OnPointerEnter")]
        [HarmonyPrefix]
        public static bool OnPointerEnter(LevelEventCategory ___levelEventCategory) {
            _lastEventCategory = ___levelEventCategory;
            SetCategoryText();
            return false;
        }
        
        [HarmonyPatch(typeof(CategoryTab), "OnPointerExit")]
        [HarmonyPrefix]
        public static bool OnPointerExit() {
            _lastEventCategory = null;
            SetCategoryText();
            return false;
        }
        
        [HarmonyPatch(typeof(CategoryTab), "SetSelected")]
        [HarmonyPostfix]
        public static void SetSelected(LevelEventCategory ___levelEventCategory) {
            _lastEventCategory = ___levelEventCategory;
            SetCategoryText();
        }
        
        [HarmonyPatch(typeof(scnEditor), "AddEvent")]
        [HarmonyPostfix]
        public static void AddEvent() {
            SetCategoryText();
        }
        
        [HarmonyPatch(typeof(scnEditor), "SelectFloor")]
        [HarmonyPostfix]
        public static void SelectFloor() {
            _lastEventCategory = null;
            SetCategoryText();
        }
        
        [HarmonyPatch(typeof(scnEditor), "RemoveEvent")]
        [HarmonyPostfix]
        public static void RemoveEvent(bool skipDecorationUpdate = false) {
            if(skipDecorationUpdate) return;
            SetCategoryText();
        }

        [HarmonyPatch(typeof(scnEditor), "UpdateDecorationObjects")]
        [HarmonyPostfix]
        public static void UpdateDecorationObjects() {
            SetCategoryText();
        }

        private static void SetCategoryText() {
            if(_lastEventCategory == null) {
                int count = ADOBase.editor.events.Count;
                ADOBase.editor.categoryText.text = $"Events ({count})";
            } else {
                int count = ADOBase.editor.events.Count(evnt => evnt.info.categories.Contains((LevelEventCategory) _lastEventCategory));
                ADOBase.editor.categoryText.text = RDString.Get($"editor.{_lastEventCategory}") + $" ({count})";
            }
        }
    }
}
