using System.Collections;
 
public class PMonitor
{  
   // Memory 
   private long startMemory = 0 ;
   private long endMemory = 0 ;
   private long usedMemory = 0 ;
   private long totalMemory = 0 ;
   private long maxMemory = 0 ;
   // time
   private DateTime startTime  ;
   private DateTime endTime    ;
   private TimeSpan usedTime   ;

   public PMonitor()
   {
      //runtime = Runtime.getRuntime() ;
       startTime = DateTime.Now ;
      //startMemory = runtime.freeMemory() ;
   }

   public Boolean done()
   {
      Boolean result = true ;
        //long memory;
        //Process[] notepads;
        //notepads = Process.GetProcessesByName("Notepad.exe");
        //memory = notepads[0].PrivateMemorySize64;
        //Console.WriteLine("Memory used: {0}.", memory);

      endTime = DateTime.Now  ;
      usedTime = endTime - startTime ;
      ///endMemory = runtime.freeMemory() ;
      //maxMemory = runtime.maxMemory() ; 
      //totalMemory = System.GC..totalMemory() ;
      //usedMemory = totalMemory - startMemory ;
      
      output() ;
      return result ;
   }

   private Boolean output()
   {
      Boolean result = true;
      Console.WriteLine(" ----------------- Processing Time information below ----------------- ") ;
      Console.WriteLine("Start time (milli seconds): "+startTime) ;
      Console.WriteLine("End   time (milli seconds): "+endTime) ;
      Console.WriteLine("Used  time (milli seconds): "+usedTime.TotalMilliseconds) ;
      Console.WriteLine("Used  time (seconds)      : "+usedTime.TotalSeconds) ;

      Console.WriteLine("\n ----------------- Processing Memory information below ----------------- ") ;
      Console.WriteLine("Start Free memory: "+startMemory) ;
      Console.WriteLine("End   Free memory: "+endMemory) ;
      Console.WriteLine("Used  Free memory: "+usedMemory) ;
      Console.WriteLine("Total      memory: "+totalMemory) ;
      Console.WriteLine("Max memory available to this JAVM: "+maxMemory) ;

      return result ;
   }

   public static void main(String []args)
   {
      PMonitor pmonitor = new PMonitor() ;
      ArrayList arrayList = new ArrayList(2) ;

      for(int i = 0; i < 500; i++)
      {
         String string1 = "                              Steve                              " ;
         arrayList.Add(string1) ;
      }


      for(int i = 0; i < 10; i++)
      {
         Console.WriteLine("Steve "+i) ;
      }

      pmonitor.done() ;

   }

}
/*
public class ElapsedMillis 
{
public static void main(String[] args) 
{
   GregorianCalendar gc1 = new GregorianCalendar(1995, 11, 1, 3, 2, 1);
   GregorianCalendar gc2 = new GregorianCalendar(1995, 11, 1, 3, 2, 2);
   // the above two dates are one second apart
   Date d1 = gc1.getTime();
   Date d2 = gc2.getTime();
   long l1 = d1.getTime();
   long l2 = d2.getTime();
   long difference = l2 - l1;
   Console.WriteLine("Elapsed milliseconds: " + difference);
}
*/


