using System.Collections.Generic;

public class GameEventOption {
    public string flavorText;
    public string tooltipText;

    public GameEventOption(string flavorText, string tooltipText) {
        this.flavorText = flavorText;
        this.tooltipText = tooltipText;
    }

    public override bool Equals(object obj) {
        return obj is GameEventOption option &&
               flavorText == option.flavorText &&
               tooltipText == option.tooltipText;
    }

    public override int GetHashCode() {
        int hashCode = 1481701121;
        hashCode = hashCode * -1521134295 + EqualityComparer<string>.Default.GetHashCode(flavorText);
        hashCode = hashCode * -1521134295 + EqualityComparer<string>.Default.GetHashCode(tooltipText);
        return hashCode;
    }
}