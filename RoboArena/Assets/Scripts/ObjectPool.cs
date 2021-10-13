using System;

using UnityEngine;

public class ObjectPool<T>
{

    private readonly T[] m_List;
    private readonly bool[] m_Available;
    private readonly Func <int, T > m_Creator;

    public ObjectPool( Func <int, T > creator, int objCount )
    {
        m_Creator = creator;
        m_Available = new bool[objCount];

        for ( int i = 0; i < m_Available.Length; i++ )
        {
            m_Available[i] = true;
        }
        m_List = new T[objCount];
    }

    private T FindUnused()
    {
        for ( int i = 0; i < m_List.Length; i++ )
        {
            if ( !m_Available[i] )
            {
                continue;
            }

            m_Available[i] = false;

            Debug.Log( "Get Pool Item: " + i );
            if(m_List[i] == null)
            {
                return m_List[i] = m_Creator(i);
            }

            return m_List[i];
        }

        throw new Exception( "Object Bool ran out of objects" );
    }

    public T Get()
    {
        return FindUnused();
    }

    public void Free( int id )
    {
        Debug.Log("Free Pool Item: " + id);
        m_Available[id] = true;
    }

}
