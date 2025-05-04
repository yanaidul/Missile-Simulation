using UnityEngine;
using HomingMissile;
using MissileSimulation.Replay;

namespace MissileSimulation.Missile
{
    public class MissileSpawner : MonoBehaviour
    {
        [SerializeField] private GameObject _homingMissilePrefab;
        [SerializeField] private GameObject _intialTarget;

        public void OnSpawnMissile(GameObject assignedTarget)
        {
            GameObject missile = Instantiate(_homingMissilePrefab.gameObject, transform.position, transform.rotation);
            missile.GetComponent<homing_missile>().target = assignedTarget;
            //missile.GetComponent<homing_missile>().target = _intialTarget;
            missile.GetComponent<homing_missile>().targetpointer.GetComponent<homing_missile_pointer>().target = assignedTarget;
            //missile.GetComponent<homing_missile>().targetpointer.GetComponent<homing_missile_pointer>().target = _intialTarget;
            missile.GetComponent<homing_missile>().initialTarget = _intialTarget;
            missile.GetComponent<homing_missile>().shooter = this.gameObject;
            missile.GetComponent<homing_missile>().usemissile();
        }
    } 
}
