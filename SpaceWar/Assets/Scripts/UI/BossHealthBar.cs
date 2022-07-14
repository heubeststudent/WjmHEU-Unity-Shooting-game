public class BossHealthBar : Status_Bar_HDR
{
    protected override void SetPercentText()
    {
        // percentText.text = string.Format("{0:P2}", targetFillAmount);
        percentText.text = targetFillAmount.ToString("P");
    }
}