using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Connection {
    public int start;
    public int end;

    public Connection() {

    }

    public Connection(int start, int end) {
        this.start = start;
        this.end = end;
    }

    public override bool Equals(object obj) {
        return obj is Connection connection &&
               start == connection.start &&
               end == connection.end;
    }

    public override int GetHashCode() {
        int hashCode = 1506791527;
        hashCode = hashCode * -1521134295 + start.GetHashCode();
        hashCode = hashCode * -1521134295 + end.GetHashCode();
        return hashCode;
    }
}
