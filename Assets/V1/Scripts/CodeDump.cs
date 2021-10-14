/**
 * Just a random file to throw maybe usable code that got irrelevant
*/


/*
//For our mesh to work it needs the corners from top to bottom in order
    private Vector2[] getMeshCorners(Vector2 start, Vector2 stop) {
        Vector2[] corners = new Vector2[4];

        if (start.x < stop.x)  { //Dragging to the left

            if (start.y > stop.y) { //Dragging top-left
                corners[0] = start;
                corners[1] = new Vector2(stop.x, start.y);
                corners[2] = new Vector2(start.x, stop.y);
                corners[3] = stop;
            }
            else { //Dragging bottom-left
                corners[0] = new Vector2(start.x, stop.y);
                corners[1] = stop;
                corners[2] = start;
                corners[3] = new Vector2(stop.x, start.y);
            }

        }
        else { //Dragging to the right
        
            if (start.y > stop.y) { //Dragging top-right
                corners[0] = new Vector2(stop.x, start.y);
                corners[0] = start;
                corners[0] = stop;
                corners[0] = new Vector2(start.x, stop.y);
            }
            else { //Dragging bottom-right
                corners[0] = stop;
                corners[0] = new Vector2(start.x, stop.y);
                corners[0] = new Vector2(stop.x, start.y);
                corners[0] = start;
            }

        }
        return corners;

    }
    private Mesh createMesh(Vector2 start, Vector2 stop) {
        Vector2[] corners = this.getMeshCorners(start, stop);

        //In 2D plain vertices == corners
        Vector3[] vertices = new Vector3[4];
        vertices[0] = corners[0];
        vertices[1] = corners[1];
        vertices[2] = corners[2];
        vertices[3] = corners[3];

        int[] triangles = {0,1,2,2,1,3};

        Mesh mesh = new Mesh();
        mesh.vertices = vertices;
        mesh.triangles = triangles;

        return mesh;
    }
*/