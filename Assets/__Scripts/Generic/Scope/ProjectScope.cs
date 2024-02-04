﻿using Arena.__Scripts.Generic.Input;
using UnityEngine;
using VContainer;
using VContainer.Unity;
namespace Arena.__Scripts.Shared.Scope
{
    public class ProjectScope : LifetimeScope
    {
        [SerializeField] private InputReader inputReader;

        protected override void Configure(IContainerBuilder builder)
        {
            builder.RegisterInstance(inputReader);
        }
    }
}