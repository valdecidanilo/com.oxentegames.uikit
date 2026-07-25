using System;
using UnityEngine;
using UnityEngine.Scripting.APIUpdating;

namespace OxenteGames.UI
{
    [MovedFrom(true, sourceNamespace: "CustomButton", sourceAssembly: "CustomButton", sourceClassName: "CustomButtonClass")]
    [Obsolete("CustomButtonClass is kept only for serialized compatibility. Use CustomButton instead.", false)]
    [AddComponentMenu("")]
    public sealed class CustomButtonClass : CustomButton
    {
    }
}
