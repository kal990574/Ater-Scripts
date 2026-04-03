using _02.Scripts._02.Ingame.Tutorial.Domain;
using System;
using System.Collections.Generic;
using UnityEngine;

namespace _02.Scripts._02.Ingame.Tutorial.Config
{
    [CreateAssetMenu(fileName = "TutorialConfig", menuName = "Ater/Tutorial/TutorialConfig")]
    public class TutorialConfigSO : ScriptableObject
    {
        [SerializeField] private List<TutorialStepEntry> _steps;                                                  
   
        public IReadOnlyList<TutorialStepEntry> Steps => _steps;         
                                                           
        public TutorialStepEntry GetStep(TutorialStepId id)
        {
            for (int i = 0; i < _steps.Count; i++)
            {
                if (_steps[i].Id == id) return _steps[i];
            }                                                
            return null;
        }                                                    
    }               

    [Serializable]
    public class TutorialStepEntry
    {
        [field: SerializeField] public TutorialStepId Id { get; private set; }                                      
        [field: SerializeField, TextArea(2, 5)] public string GuideText { get; private set; }                         
        [field: SerializeField] public TutorialStepId ChainFrom { get; private set; }                          
        [field: SerializeField] public bool IsOverlay { get; private set; }                                           
    }               
}