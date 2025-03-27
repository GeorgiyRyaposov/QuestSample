using UnityEngine;

namespace Code.Scripts.Configs
{
    public interface IConfigsContainer
    {
        void UpdateItems(IAssetsFinder finder);
    }

    public interface IAssetsFinder
    {
        T[] GetAssets<T>() where T : Object;
    }
}