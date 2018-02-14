namespace ComposableArchitecture
{
    internal class ToggleViewModel : ViewModel
    {
        private string label;
        private string onLabel;
        private string offLabel;
        private ISubject<bool> isOn;

        public ToggleViewModel()
        {
        }

        public ToggleViewModel(string label, string onLabel, string offLabel, ISubject<bool> isOn)
        {
            this.label = label;
            this.onLabel = onLabel;
            this.offLabel = offLabel;
            this.isOn = isOn;
        }
    }
}