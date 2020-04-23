using System;
using UnityEngine;

public class ConsumableItem : Item {

    public int healthHealAmount;
    public int hungerHealAmount;
    public int thirstHealAmount;

    public override bool Use() {
        FPSCharacterController player = GetPlayerController();
        int healthHeal = healthHealAmount;
        if (healthHealAmount < 0) {
            healthHeal = 0;
            player.TakeDamage(Math.Abs(healthHealAmount));
        }
        int hungerHeal = hungerHealAmount;
        if (hungerHealAmount < 0) {
            hungerHeal = 0;
            player.Famish(Math.Abs(hungerHealAmount));
        }
        int thirstHeal = thirstHealAmount;
        if (thirstHealAmount < 0) {
            thirstHeal = 0;
            player.Dehydrate(Math.Abs(thirstHealAmount));
        }

        player.Heal(healthHeal, hungerHeal, thirstHeal);

        Destroy(gameObject);
        
        return true;
    }
}
