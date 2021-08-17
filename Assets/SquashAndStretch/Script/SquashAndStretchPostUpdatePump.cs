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
  internal class SquashAndStretchPostUpdatePump : MonoBehaviour
  {
    private void LateUpdate()
    {
      SquashAndStretchManager.HandleSelection();
      SquashAndStretchManager.PostUpdate();
    }
  }
}

