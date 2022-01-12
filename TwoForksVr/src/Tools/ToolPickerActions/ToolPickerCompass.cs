namespace TwoForksVr.Tools.ToolPickerActions
{
    public class ToolPickerCompass : ToolPickerAction
    {
        private vgMapController mapController;

        protected override void OnInitialize()
        {
            mapController = vgPlayerController.playerGameObject.GetComponent<vgMapController>();
        }

        protected override void OnSelect()
        {
            mapController.OnToggleCompass();
        }

        protected override void OnDeselect()
        {
            mapController.OnToggleCompass();
        }

        protected override bool IsEquipped()
        {
            return mapController.compassEquipped;
        }
    }
}