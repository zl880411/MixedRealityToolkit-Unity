﻿// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

using System.Collections.Generic;
using UnityEngine;

namespace Microsoft.MixedReality.Toolkit.Examples.Demos.EyeTracking
{
    [RequireComponent(typeof(ParticleSystem))]
    public class ParticleHeatmap : MonoBehaviour
    {
        //public Material heatmapShader;
        public Gradient colorGradient;
        public int maxNumberOfParticles = 100;
        public float minParticleSize = .01f;
        public float maxParticleSize = 0.5f;

        private int particleDecalDataIndex;

        private ParticleSystem particleSys;
        private ParticleSystem.EmissionModule emissionModule;
        private List<ParticleHeatmap_ParticleData> particleData;

        private void Start()
        {
            // Initialize particle data handlers
            particleSys = GetComponent<ParticleSystem>();
            emissionModule = particleSys.emission;
            particleData = new List<ParticleHeatmap_ParticleData>();
        }

        public void SetParticle(Vector3 pos)
        {
            if (particleDecalDataIndex >= maxNumberOfParticles)
            {
                particleDecalDataIndex = 0;
            }

            ParticleHeatmap_ParticleData newParticle = new ParticleHeatmap_ParticleData();
            newParticle.position = pos;
            newParticle.radiusInMeter = Random.Range(minParticleSize, maxParticleSize);

            //Vector3 particleRotationEuler = Quaternion.LookRotation(CameraCache.Main.transform.position - pos).eulerAngles;
            //particleRotationEuler.z = Random.Range(0, 360);
            //particleData[particleDecalDataIndex].rotation = particleRotationEuler;
            //particleData[particleDecalDataIndex].color = Color.red; // colorGradient.Evaluate(0f);
            //newParticle.color = colorGradient.Evaluate(Random.Range(0f, 1f));
            //newParticle.color = colorGradient.Evaluate(DetermineNormalizedIntensity(newParticle.position, newParticle.radiusInMeter)); // colorGradient.Evaluate(Random.Range(0f, 1f));

            if (particleDecalDataIndex >= particleData.Count)
            {
                particleData.Add(newParticle);
            }
            else
            {
                particleData[particleDecalDataIndex] = newParticle;
            }

            particleDecalDataIndex++;

            //UpdateColorForAllParticles();
        }

        public Vector3? GetPositionOfParticle(int index)
        {
            if ((index >= particleData.Count) || (index < 0))
                return null;
            return particleData[index].position;
        }

        public float colorScaleTweaker = 0.1f;
        private float DetermineNormalizedIntensity(Vector3 pos, float radius)
        {
            float tmpIntensity = 0;

            foreach (ParticleHeatmap_ParticleData point in particleData)
            {
                if (pos != point.position)
                {
                    float dist = Vector3.Distance(pos, point.position);
                    float tmpInt2 = 1 - saturate(dist / radius);
                    tmpIntensity += tmpInt2 * colorScaleTweaker;
                }
            }
            float finalIntensity = saturate(tmpIntensity);
            return finalIntensity;
        }

        private void UpdateColorForAllParticles()
        {
            foreach (ParticleHeatmap_ParticleData particle in particleData)
            {
                particle.color = colorGradient.Evaluate(DetermineNormalizedIntensity(particle.position, particle.radiusInMeter));
            }
        }

        float saturate(float val)
        {
            if (val > 1)
                return 1;
            if (val < 0)
                return 0;
            return val;
        }

        public void DisplayParticles()
        {
            ParticleSystem.Particle[] particleArray = new ParticleSystem.Particle[particleData.Count];
            for (int i = 0; i < particleData.Count; i++)
            {
                particleArray[i].position = particleData[i].position;
                particleArray[i].startColor = particleData[i].color;
                particleArray[i].startSize = particleData[i].radiusInMeter;
                //particleArray[i].rotation3D = particleData[i].rotation;
            }

            particleSys.SetParticles(particleArray, particleArray.Length);
        }

        public void ShowHeatmap()
        {
            emissionModule.enabled = true;
        }

        public void HideHeatmap()
        {
            emissionModule.enabled = false;
        }
    }
}