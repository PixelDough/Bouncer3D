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
  internal abstract class SquashAndStretchModeLogic
  {
    internal abstract void Reboot(SquashAndStretch squashAndStretch);

    internal abstract void Update
    (
      SquashAndStretch squashAndStretch, 
      out Vector3 position, 
      out Vector3 stretchAxis, 
      out float stretch
    );

    internal abstract void Move(Vector3 offset);
  }
}
