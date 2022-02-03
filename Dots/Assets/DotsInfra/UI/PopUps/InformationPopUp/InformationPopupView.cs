using TMPro;
using UnityEngine;

namespace Dots.Infra.UI
{
    public interface IInformationPopupView : IPopUpView
    {
        void SetData(InformationPopUpData data);
    }

    public class InformationPopupView : PopupView, IInformationPopupView
    {
        [SerializeField] private TextMeshProUGUI _title;
        [SerializeField] private TextMeshProUGUI _description;

        public void SetData(InformationPopUpData data)
        {
            _title.text = data.Title;
            _description.text = data.Description;
        }
    }
}