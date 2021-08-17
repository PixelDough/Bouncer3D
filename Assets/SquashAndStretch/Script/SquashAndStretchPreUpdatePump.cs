/******************************************************************************/
/*
  Project   - Squash And Stretch Kit
  Publisher - Long Bunny Labs
              http://LongBunnyLabs.com
  Author    - Ming-Lun "Allen" Chou
              http://AllenChou.net
*/
/******************************************************************************/

using UnityEngine;

namespace SquashAndStretchKit
{
  internal class SquashAndStretchPreUpdatePump : MonoBehaviour
  {
    private void FixedUpdate()
    {
      SquashAndStretchManager.PreUpdate(true);
    }

    private void Update()
    {
      SquashAndStretchManager.PreUpdate(false);
    }
  }
}

