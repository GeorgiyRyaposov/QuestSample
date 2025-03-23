using UnityEngine;

namespace Code.Scripts.Services.Common
{
    //[CreateAssetMenu(menuName = "Data/Services/ServicesContainer", fileName = "ServicesContainer")]
    public class ServicesContainer : ScriptableObject
    {
        public ScriptableService[] ScriptableServices = new ScriptableService[0];
    }
}